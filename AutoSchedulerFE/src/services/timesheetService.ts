import type { ActivityRequirements } from "@/classes/activity";
import type { GeneratorRequirements, Timesheet, TimeslotPlacementChange } from "@/classes/timesheet";
import axios, { AxiosError, type AxiosResponse } from "axios";
import { axiosInstance } from "./interceptors/authInterceptor";

export function fetchTimesheetForGroup (groupId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/Timesheet/group/${groupId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

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

export function regenerateNewTimesheet (timeslotPlacementChange:TimeslotPlacementChange)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/regenerate`, timeslotPlacementChange)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchAvailableSpaceForTimeslot (timeslotPlacementChange:TimeslotPlacementChange)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/timeslot/available`, timeslotPlacementChange)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchConflictingTimeslots (timeslotPlacementChange:TimeslotPlacementChange)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/timeslot/conflicting`, timeslotPlacementChange)
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