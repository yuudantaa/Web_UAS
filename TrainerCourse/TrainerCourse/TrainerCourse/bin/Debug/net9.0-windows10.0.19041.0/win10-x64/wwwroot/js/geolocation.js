// wwwroot/js/geolocation.js
export async function getCurrentLocation() {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject(new Error('Geolocation is not supported by your browser'));
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (position) => {
                resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy,
                    timestamp: new Date(position.timestamp)
                });
            },
            (error) => {
                let errorMessage;
                switch (error.code) {
                    case error.PERMISSION_DENIED:
                        errorMessage = "User denied the request for Geolocation.";
                        break;
                    case error.POSITION_UNAVAILABLE:
                        errorMessage = "Location information is unavailable.";
                        break;
                    case error.TIMEOUT:
                        errorMessage = "The request to get user location timed out.";
                        break;
                    default:
                        errorMessage = "An unknown error occurred.";
                        break;
                }
                reject(new Error(errorMessage));
            },
            {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 0
            }
        );
    });
}

export async function watchLocation(callback) {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject(new Error('Geolocation is not supported'));
            return;
        }

        const watchId = navigator.geolocation.watchPosition(
            (position) => {
                callback({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy,
                    timestamp: new Date(position.timestamp)
                });
            },
            (error) => {
                reject(error);
            },
            {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 0
            }
        );

        resolve(watchId);
    });
}

export function clearWatch(watchId) {
    if (navigator.geolocation) {
        navigator.geolocation.clearWatch(watchId);
    }
}

export function checkGeolocationPermission() {
    return new Promise((resolve) => {
        if (!navigator.permissions || !navigator.permissions.query) {
            resolve('prompt');
            return;
        }

        navigator.permissions.query({ name: 'geolocation' })
            .then((permissionStatus) => {
                resolve(permissionStatus.state);
            })
            .catch(() => {
                resolve('prompt');
            });
    });
}

export async function requestGeolocationPermission() {
    try {
        // Permissions API might not be available in all browsers
        if (navigator.permissions && navigator.permissions.query) {
            const permissionStatus = await navigator.permissions.query({ name: 'geolocation' });
            return permissionStatus.state;
        } else {
            // Fallback: try to get location
            const position = await getCurrentLocation();
            return 'granted';
        }
    } catch (error) {
        return 'denied';
    }
}