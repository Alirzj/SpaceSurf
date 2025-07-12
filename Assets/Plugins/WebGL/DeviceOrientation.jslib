var DeviceOrientationPlugin = {
    $deviceOrientation: {
        // Store orientation data
        beta: 0,    // Front-back tilt
        gamma: 0,   // Left-right tilt
        alpha: 0,   // Compass heading
        supported: false,
        permissionGranted: false,
        
        // Initialize the plugin
        init: function() {
            // Check if DeviceOrientationEvent is supported
            if (typeof DeviceOrientationEvent !== 'undefined') {
                deviceOrientation.supported = true;
                
                // Set up event listener
                window.addEventListener('deviceorientation', function(event) {
                    if (event.beta !== null && event.gamma !== null && event.alpha !== null) {
                        deviceOrientation.beta = event.beta;
                        deviceOrientation.gamma = event.gamma;
                        deviceOrientation.alpha = event.alpha;
                        deviceOrientation.permissionGranted = true;
                    }
                });
                
                console.log('Device orientation event listener added');
            } else {
                console.log('DeviceOrientationEvent not supported');
            }
        },
        
        // Request permission for iOS 13+ Safari
        requestPermission: function() {
            if (typeof DeviceOrientationEvent !== 'undefined' && 
                typeof DeviceOrientationEvent.requestPermission === 'function') {
                
                DeviceOrientationEvent.requestPermission()
                    .then(function(permissionState) {
                        if (permissionState === 'granted') {
                            deviceOrientation.permissionGranted = true;
                            console.log('Device orientation permission granted');
                        } else {
                            console.log('Device orientation permission denied');
                        }
                    })
                    .catch(function(error) {
                        console.error('Error requesting device orientation permission:', error);
                    });
            } else {
                // For non-iOS devices or older iOS versions, permission is automatic
                deviceOrientation.permissionGranted = true;
            }
        }
    },
    
    // Unity callable functions
    RequestDeviceOrientationPermission: function() {
        if (!deviceOrientation.supported) {
            deviceOrientation.init();
        }
        deviceOrientation.requestPermission();
    },
    
    IsDeviceOrientationSupported: function() {
        if (!deviceOrientation.supported) {
            deviceOrientation.init();
        }
        return deviceOrientation.supported;
    },
    
    GetDeviceOrientationBeta: function() {
        return deviceOrientation.beta;
    },
    
    GetDeviceOrientationGamma: function() {
        return deviceOrientation.gamma;
    },
    
    GetDeviceOrientationAlpha: function() {
        return deviceOrientation.alpha;
    },
    
    HasDeviceOrientationPermission: function() {
        return deviceOrientation.permissionGranted;
    }
};

// Auto-merge into library
autoAddDeps(DeviceOrientationPlugin, '$deviceOrientation');
mergeInto(LibraryManager.library, DeviceOrientationPlugin);