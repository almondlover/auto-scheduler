import type { Group, Member } from "@/classes/group";
import axios, { AxiosError, type AxiosResponse } from "axios";

export function fetchGroupsForOrganization (organizationId:number)
{
    return axios.get(`${axios.defaults.baseURL}/MemberGroup/organization/${organizationId}/all`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchOrganizations ()
{
    return axios.get(`${axios.defaults.baseURL}/MemberGroup/organization/all`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchOrganization (organizationId:number)
{
    return axios.get(`${axios.defaults.baseURL}/MemberGroup/organization/${organizationId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function saveGroup (group:Group)
{
    return axios.post(`${axios.defaults.baseURL}/MemberGroup/new`, group)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function createMember (member:Member)
{
    return axios.post(`${axios.defaults.baseURL}/MemberGroup/member/new`, member)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};