import axios from "axios";

export const axiosInstance = axios.create();

axiosInstance.interceptors.request.use(config => {
    config.headers.authorization = `Bearer ${localStorage.getItem('userToken')}`;
    return config;
}, error => {
    return Promise.reject(error);
});
