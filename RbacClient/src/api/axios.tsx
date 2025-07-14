import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5107/api",
});

export default api;
