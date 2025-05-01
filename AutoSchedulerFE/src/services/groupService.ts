import axios, { AxiosError, type AxiosResponse } from "axios";

export function fetchGroupsForOrganization (organizationId:number)
{
    return axios.get(`${axios.defaults.baseURL}/organization/${organizationId}/all`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
}