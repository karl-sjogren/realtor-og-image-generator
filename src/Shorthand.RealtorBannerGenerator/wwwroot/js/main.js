import { waitForImage } from './image-loader.js';

const params = new URLSearchParams(location.search.substring(1));

const property = await fetch('/api/property/' + params.get('identifier'))
  .then(response => response.json());

const propertyBackgroundImageElement = document.querySelector('.property-background-image');
propertyBackgroundImageElement.style.backgroundImage = `url('${property.propertyImageUrl}')`;

const realtorBackgroundImageElement = document.querySelector('.realtor-background-image');
realtorBackgroundImageElement.style.backgroundImage = `url('${property.realtorImageUrl}')`;

const realtorNameElement = document.querySelector('.realtor-name');
realtorNameElement.textContent = property.realtorName;

const streetAddressElement = document.querySelector('.street-address');
streetAddressElement.textContent = property.streetAddress;

const cityElement = document.querySelector('.city');
cityElement.textContent = property.city;

const formattedPrice = new Intl.NumberFormat('sv-SE',
  { style: 'currency', currency: 'SEK', minimumFractionDigits: 0 }
).format(property.startingPrice);

const priceElement = document.querySelector('.price');
priceElement.textContent = !property.startingPrice ? 'Inget pris satt' : formattedPrice;

Promise.all([
  waitForImage(property.propertyImageUrl),
  waitForImage(property.realtorImageUrl)
])
  .then(() => {
    // This signals that the screenshot can be taken
    console.log('Document ready');
    window.documentReady = true;
  });
