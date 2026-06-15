import type { ActivityRequirements } from "@/classes/activity";
import type { GeneratorRequirements, Timesheet, TimeslotPlacementChange, TimeslotRearrangement } from "@/classes/timesheet";
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

export function regenerateNewTimesheet (timeslotRearrangement:TimeslotRearrangement)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/regenerate`, timeslotRearrangement)
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

export function fetchAvailableHallsForTimeslot (timeslotPlacementChange:TimeslotPlacementChange)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/timeslot/halls/available`, timeslotPlacementChange)
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

export function updateTimesheet (timesheet:Timesheet)
{
    return axiosInstance.put(`${axios.defaults.baseURL}/Timesheet/update`, timesheet)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};
export function activateTimesheet (timesheetId:number)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Timesheet/${timesheetId}/activate`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};