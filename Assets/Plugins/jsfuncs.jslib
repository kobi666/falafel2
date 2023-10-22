mergeInto(LibraryManager.library, {

  Vibrate: function () {
    console.log("vibrate");
    let canVibrate = false;
    try {
    if('vibrate' in navigator) {
    canVibrate = true;
    }
    if (navigator.vibrate) {
    navigator.vibrate(50);
    }
    }
    catch (error) {
      console.log("error in vibrating");
    }
  },
  Vibrate2: function () {
    console.log("vibrate");
    let canVibrate = false;
    try {
    if('vibrate' in navigator) {
    canVibrate = true;
    }
    if (navigator.vibrate) {
    navigator.vibrate(150);
    }
    }
    catch (error) {
      console.log("error in vibrating");
    }
  }
});