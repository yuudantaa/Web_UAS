// wwwroot/js/camera.js - Updated version
let stream = null;
let videoElement = null;
let canvasElement = null;

export async function startCamera() {
    try {
        videoElement = document.getElementById('cameraPreview');

        // If no video element with that ID exists, create one
        if (!videoElement) {
            videoElement = document.createElement('video');
            videoElement.id = 'cameraPreview';
            videoElement.autoplay = true;
            videoElement.playsinline = true;
            videoElement.style.width = '100%';
            videoElement.style.height = 'auto';
            videoElement.style.display = 'none'; // Hide by default
            document.body.appendChild(videoElement);
        }

        canvasElement = document.createElement('canvas');

        // Get available video devices to find the best camera
        const devices = await navigator.mediaDevices.enumerateDevices();
        const videoDevices = devices.filter(device => device.kind === 'videoinput');

        let constraints = {
            video: {
                width: { ideal: 1280 },
                height: { ideal: 720 },
                facingMode: 'environment' // Try back camera first, then fallback
            },
            audio: false
        };

        // If on desktop/laptop, don't specify facingMode
        const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
        if (!isMobile) {
            constraints.video = {
                width: { ideal: 1280 },
                height: { ideal: 720 }
            };
        }

        stream = await navigator.mediaDevices.getUserMedia(constraints);
        videoElement.srcObject = stream;

        return true;
    } catch (error) {
        console.error('Error accessing camera:', error);

        // Fallback to simpler constraints
        try {
            stream = await navigator.mediaDevices.getUserMedia({
                video: true,
                audio: false
            });
            videoElement.srcObject = stream;
            return true;
        } catch (fallbackError) {
            console.error('Fallback also failed:', fallbackError);
            return false;
        }
    }
}

export function stopCamera() {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        stream = null;
    }

    if (videoElement) {
        videoElement.srcObject = null;
    }
}

export function capturePhoto() {
    try {
        if (!videoElement || !stream) {
            console.error('Camera not active');
            return null;
        }

        if (!canvasElement) {
            canvasElement = document.createElement('canvas');
        }

        // Wait for video to be ready
        if (videoElement.videoWidth === 0 || videoElement.videoHeight === 0) {
            console.warn('Video not ready yet, using default size');
            canvasElement.width = 640;
            canvasElement.height = 480;
        } else {
            canvasElement.width = videoElement.videoWidth;
            canvasElement.height = videoElement.videoHeight;
        }

        const context = canvasElement.getContext('2d');

        // Draw the current video frame to canvas
        context.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height);

        // Convert to data URL (JPEG with 85% quality)
        return canvasElement.toDataURL('image/jpeg', 0.85);
    } catch (error) {
        console.error('Error capturing photo:', error);
        return null;
    }
}