async function searchProperty() {

    let city = document.getElementById("city").value;

    const response = await fetch(`/api/propertysearch?city=${city}`);

    const data = await response.json();

    console.log(data);
}