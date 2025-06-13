import type { Activity, ActivityRequirements } from "@/classes/activity";
import axios, { AxiosError, type AxiosResponse } from "axios";

export function createActivityRequirement (requirement:ActivityRequirements)
{
    //temporary cast to db model, should use dtos
    const requirementModel = {
        duration:requirement.duration,
        activityId:requirement.activity.id,
        groupId:requirement.group?.id
    };
    return axios.post(`${axios.defaults.baseURL}/Activity/requirement/new`, requirementModel)
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
    return axios.post(`${axios.defaults.baseURL}/Activity/new`, activity)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )

}

export function fetchActivitiesForOrganization (organizationId:number)
{
    return axios.get(`${axios.defaults.baseURL}/Activity/organization/${organizationId}`)
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
    return axios.get(`${axios.defaults.baseURL}/Activity/requirements/group/${groupId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};
