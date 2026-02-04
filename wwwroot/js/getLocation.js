/**
 * Gets the user's current geographical location using the longitude and latitude.
 * If successful, it calls the tansmitPosition function with the position data to the browse controller.
 * If there's an error, it calls the showError function and displays an error message.
 */
function getLocation() {

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(tansmitPosition, showError);
    }
    else {
        console.log("Geolocation is not supported by this browser.");
    }
};

/**
 * transmists the position data to the browse controller.
 * @param {object} position - The position object containing latitude and longitude.
 */
function tansmitPosition(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;

    var userLocation = {
        Latitude: latitude,
        Longitude: longitude
    };

    fetch('/Browse/UserLocation/', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userLocation)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('User location transmitted successfully:', data);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });


}

/** * Displays an error message based on the error code received.
 * @param {object} error - The error object containing the error code.
//  */
function showError(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            console.log("User denied the request for Geolocation.");
            break;
        case error.POSITION_UNAVAILABLE:
            console.log("Location information is unavailable.");
            break;
        case error.TIMEOUT:
            console.log("The request to get user location timed out.");
            break;
        case error.UNKNOWN_ERROR:
            console.log("An unknown error occurred.");
            break;
    }
}

