import type { ActivityRequirements } from "@/classes/activity";
import type { GeneratorRequirements } from "@/classes/timesheet";
import axios, { AxiosError, type AxiosResponse } from "axios";

export function generateNewTimesheet (generatorRequirements:GeneratorRequirements)
{
    return axios.post(`${axios.defaults.baseURL}/Timesheet/generate`, generatorRequirements)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};