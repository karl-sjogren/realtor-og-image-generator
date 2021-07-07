export let waitForImage = function waitForImage(url) {
  return new Promise(resolve => {
    const image = new Image();
    image.onload = function() {
      resolve();
    }
    image.src = url;
    if (image.complete) {
      resolve();
    }
  });
}
