import type { LoginModel, RegisterModel } from "@/classes/user";
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

export function registerUser (registerModel:RegisterModel)
{
    //should use interceptors
    return axios.post(`${axios.defaults.baseURL}/User/register`, registerModel)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};