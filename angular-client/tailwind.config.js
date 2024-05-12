/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      animation: {
        "fade-in": "fade-in 0.5s ease-out",
        "fade-out": "fade-out 0.5s ease-out",
      },
      keyframes: {
        "fade-in": {
          "0%": { opacity: "0" },
          "100%": { opacity: "1" },
        },
        "fade-out": {
          "0%": { opacity: "1" },
          "100%": { opacity: "0" },
        },
      },
      colors: {
        "primary-blue-500": "#326273",
        "primary-blue-400": "#4B91AA",
        "primary-blue-300": "#5C9EAD",
        "primary-blue-200": "#85B6C1",
        "primary-blue-100": "#C9DFE4",
        "primary-orange": "#E39774",
        "primary-gray-light": "#EEEEEE",
      },
    },
  },
  plugins: [],
};
