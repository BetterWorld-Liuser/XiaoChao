var hotkeys = require('hotkeys-js')

hotkeys('f5', function(event, handler){
  // Prevent the default refresh event under WINDOWS system
  event.preventDefault() 
  alert('you pressed F5!') 
});