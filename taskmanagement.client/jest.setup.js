import "@testing-library/jest-dom";

beforeEach(() => { global.fetch = jest.fn(); });
afterEach(() => { jest.resetAllMocks(); });

if (!process.env.VITE_API_URL) process.env.VITE_API_URL = "";

global.ResizeObserver = class { observe(){} unobserve(){} disconnect(){} };
window.matchMedia = window.matchMedia || function () {
  return { matches:false, addListener(){}, removeListener(){}, addEventListener(){}, removeEventListener(){}, dispatchEvent(){} };
};
