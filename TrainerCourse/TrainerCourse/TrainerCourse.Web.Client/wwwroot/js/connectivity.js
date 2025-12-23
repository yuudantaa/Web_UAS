// Connectivity JavaScript for Blazor Web
window.TrainerCourse = window.TrainerCourse || {};

window.TrainerCourse.connectivity = {
    // Initialize connectivity monitoring
    init: function (dotNetHelper) {
        // Store the .NET helper
        this.dotNetHelper = dotNetHelper;

        // Listen to online/offline events
        window.addEventListener('online', () => {
            this.checkAndNotify(true);
        });

        window.addEventListener('offline', () => {
            dotNetHelper.invokeMethodAsync('UpdateConnectionStatus', false);
        });

        // Initial check
        this.checkAndNotify(navigator.onLine);
    },

    // Check connection by pinging a reliable endpoint
    checkConnection: async function () {
        try {
            // Try multiple endpoints for reliability
            const endpoints = [
                'https://www.google.com/favicon.ico',
                'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css',
                'https://localhost:7285/health' // Your API health endpoint
            ];

            // Try the first endpoint
            const response = await fetch(endpoints[0], {
                method: 'HEAD',
                mode: 'no-cors',
                cache: 'no-cache'
            });

            return true;
        } catch (error) {
            console.log('Connectivity check failed:', error);
            return navigator.onLine;
        }
    },

    // Check and notify .NET about connection status
    checkAndNotify: async function (browserStatus) {
        if (!this.dotNetHelper) return;

        try {
            // Perform actual network check
            const actualStatus = await this.checkConnection();
            this.dotNetHelper.invokeMethodAsync('UpdateConnectionStatus', actualStatus);
        } catch (error) {
            // Fallback to browser status
            this.dotNetHelper.invokeMethodAsync('UpdateConnectionStatus', browserStatus);
        }
    }
};