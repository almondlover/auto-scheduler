import type { ActivityRequirements } from "@/classes/activity";
import type { GeneratorRequirements, Timesheet } from "@/classes/timesheet";
import axios, { AxiosError, type AxiosResponse } from "axios";
import { axiosInstance } from "./interceptors/authInterceptor";

export function generateNewTimesheet (generatorRequirements:GeneratorRequirements)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/generate`, generatorRequirements)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function createTimesheet (timesheet:Timesheet)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/new`, timesheet)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};