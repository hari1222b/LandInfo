"""
Land Price Predictor — FastAPI Microservice
Run with:  uvicorn price_predictor:app --reload --port 8000
"""
from __future__ import annotations

import os
import uvicorn                                     # type: ignore[import]
from contextlib import asynccontextmanager
from fastapi import FastAPI, HTTPException          # type: ignore[import]
from pydantic import BaseModel                     # type: ignore[import]
import pandas as pd                                # type: ignore[import]
import numpy as np                                 # type: ignore[import]
from sklearn.ensemble import RandomForestRegressor  # type: ignore[import]
import joblib                                      # type: ignore[import]

MODEL_PATH = "land_price_model.pkl"
model = None


# ── Input schema ──────────────────────────────────────────────────────────────
class PredictionRequest(BaseModel):
    latitude:          float | None = None
    longitude:         float | None = None
    land_size:         float
    road_access:       bool
    water_facility:    bool
    school_distance:   float
    hospital_distance: float


# ── Synthetic dataset ─────────────────────────────────────────────────────────
def generate_synthetic_data(num_samples: int = 1000) -> pd.DataFrame:
    np.random.seed(42)

    land_sizes        = np.random.uniform(500, 10000, num_samples)
    road_accesses     = np.random.choice([0, 1], num_samples, p=[0.2, 0.8])
    water_facilities  = np.random.choice([0, 1], num_samples, p=[0.3, 0.7])
    school_distances  = np.random.uniform(0.1, 15, num_samples)
    hospital_distances = np.random.uniform(0.5, 20, num_samples)

    # Price formula (synthetic rules — Indian Rupee scale)
    prices  = land_sizes * 100
    prices += road_accesses    * (land_sizes * 20)
    prices += water_facilities * (land_sizes * 15)
    prices -= school_distances  * 1000
    prices -= hospital_distances * 2000

    # ±5 % market noise
    prices *= np.random.uniform(0.95, 1.05, num_samples)

    return pd.DataFrame({
        'land_size':         land_sizes,
        'road_access':       road_accesses,
        'water_facility':    water_facilities,
        'school_distance':   school_distances,
        'hospital_distance': hospital_distances,
        'price':             prices,
    })


# ── Model train / load ────────────────────────────────────────────────────────
def train_model() -> RandomForestRegressor:
    print("Generating synthetic dataset …")
    df = generate_synthetic_data()

    X = df[['land_size', 'road_access', 'water_facility',
             'school_distance', 'hospital_distance']]
    y = df['price']

    print("Training RandomForestRegressor …")
    regressor = RandomForestRegressor(n_estimators=100, random_state=42)
    regressor.fit(X, y)

    joblib.dump(regressor, MODEL_PATH)
    print("Model saved to", MODEL_PATH)
    return regressor


# ── App lifespan (replaces deprecated @app.on_event) ─────────────────────────
@asynccontextmanager
async def lifespan(app: FastAPI):
    global model
    if os.path.exists(MODEL_PATH):
        print("Loading existing model …")
        model = joblib.load(MODEL_PATH)
    else:
        model = train_model()
    yield  # server runs here


app = FastAPI(title="Land Price Predictor API", lifespan=lifespan)


# ── Predict endpoint ──────────────────────────────────────────────────────────
@app.post("/predict")
def predict_price(request: PredictionRequest):
    if model is None:
        raise HTTPException(status_code=500, detail="Model not loaded.")

    features = pd.DataFrame([{
        'land_size':         request.land_size,
        'road_access':       1 if request.road_access else 0,
        'water_facility':    1 if request.water_facility else 0,
        'school_distance':   request.school_distance,
        'hospital_distance': request.hospital_distance,
    }])

    try:
        predicted_price: float = float(model.predict(features)[0])

        # 12-month simulated trend (starts 10–15 % lower → rises to prediction)
        months = ["Jan","Feb","Mar","Apr","May","Jun",
                  "Jul","Aug","Sep","Oct","Nov","Dec"]
        start_factor  = float(np.random.uniform(0.85, 0.90))
        current_factor = start_factor
        trend_data = []

        for i in range(12):
            current_factor += (1.0 - start_factor) / 11
            volatility      = float(np.random.uniform(0.98, 1.02))
            val: float      = predicted_price * current_factor * volatility
            if i == 11:
                val = predicted_price

            trend_data.append({
                "month": months[i],
                "price": float(round(val, 2)),   # explicit float avoids type narrowing
            })

        return {
            "predicted_price": float(round(predicted_price, 2)),
            "trend_data":      trend_data,
        }

    except Exception as exc:
        raise HTTPException(status_code=500, detail=str(exc)) from exc


# ── Dev entry-point ───────────────────────────────────────────────────────────
if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)
