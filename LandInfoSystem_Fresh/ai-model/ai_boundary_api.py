import flask
from flask import request, jsonify
import numpy as np
import cv2
import base64
from flask_cors import CORS

app = flask.Flask(__name__)
CORS(app)

@app.route('/detect-boundary', methods=['POST'])
def detect_boundary():
    """
    Detects land boundaries from a satellite image or coordinates.
    Input: { "image": "base64_str", "lat": float, "lng": float }
    Output: { "status": "success", "boundary": [[lat, lng], ...] }
    """
    data = request.json
    if not data:
        return jsonify({"status": "error", "message": "No data provided"}), 400

    lat = data.get('lat')
    lng = data.get('lng')
    image_b64 = data.get('image')

    # MOCK AI Logic: Create a square polygon around the location
    # Real logic would use OpenCV/Segmentation on the image_b64
    
    # 0.0005 deg is approx 50m
    offset = 0.0005 
    
    mock_boundary = [
        [lat + offset, lng - offset],
        [lat + offset, lng + offset],
        [lat - offset, lng + offset],
        [lat - offset, lng - offset]
    ]

    return jsonify({
        "status": "success",
        "boundary": mock_boundary,
        "message": "AI detected approximate boundary. Please adjust manually if needed."
    })

if __name__ == '__main__':
    # Running on port 5001 to avoid conflict with main API (5000)
    app.run(host='0.0.0.0', port=5001)
