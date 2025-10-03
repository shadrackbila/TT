/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,html}",
    "./wwwroot/js/**/*.js"
  ],

  theme: {
    extend: {
      colors: {
        primary: '#4CAF50',
        secondary: '#FFC107',
        accent: '#FF5722',
        dark: '#263238',
        light: '#ECEFF1',
      }
    }
  },
  plugins: [],
}

