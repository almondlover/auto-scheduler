import type { ActivityRequirements } from "@/classes/activity";
import axios, { AxiosError, type AxiosResponse } from "axios";

export function generateNewTimesheet (requirements:ActivityRequirements[])
{
    return axios.post(`${axios.defaults.baseURL}/Timesheet/generate`, requirements)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};