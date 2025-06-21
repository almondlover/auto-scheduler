import type { Activity, ActivityRequirements, Hall } from "@/classes/activity";
import axios, { AxiosError, type AxiosResponse } from "axios";
import { axiosInstance } from "./interceptors/authInterceptor";

export function createActivityRequirement (requirement:ActivityRequirements)
{
    console.log(requirement);
    return axiosInstance.post(`${axios.defaults.baseURL}/Activity/requirement/new`, requirement)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function saveActivity (activity:Activity)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Activity/new`, activity)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )

};

export function createHall (hall:Hall)
{
    return axiosInstance.post(`${axios.defaults.baseURL}/Activity/halls/new`, hall)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )

};

export function fetchActivitiesForOrganization (organizationId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/Activity/organization/${organizationId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function fetchActivityRequirementsForGroup (groupId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/Activity/requirements/group/${groupId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function fetchHallTypes ()
{
    return axiosInstance.get(`${axios.defaults.baseURL}/Activity/hallTypes/all`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function deleteActivity (activityId:number)
{
    return axiosInstance.delete(`${axios.defaults.baseURL}/Activity/delete/${activityId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function deleteHall (hallId:number)
{
    return axiosInstance.delete(`${axios.defaults.baseURL}/Activity/hall/delete/${hallId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};
