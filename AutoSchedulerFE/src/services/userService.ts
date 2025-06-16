import type { LoginModel } from "@/classes/user";
import axios, { AxiosError, type AxiosResponse } from "axios";

export function loginUser (loginModel:LoginModel)
{
    return axios.post(`${axios.defaults.baseURL}/User/login`, loginModel)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function registerUser (loginModel:LoginModel)
{
    //should use interceptors
    return axios.post(`https://localhost:7002/register`, loginModel)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};