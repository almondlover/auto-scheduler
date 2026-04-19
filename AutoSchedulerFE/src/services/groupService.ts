import type { Group, Member } from "@/classes/group";
import axios, { AxiosError, type AxiosResponse } from "axios";
import { axiosInstance } from "./interceptors/authInterceptor";

export function fetchGroupsForOrganization (organizationId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/MemberGroup/organization/${organizationId}/all`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchRootGroupsForOrganization (organizationId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/MemberGroup/organization/${organizationId}/all/root`)
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
    return axiosInstance.get(`${axios.defaults.baseURL}/MemberGroup/organization/all`)
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
    return axiosInstance.get(`${axios.defaults.baseURL}/MemberGroup/organization/${organizationId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                Promise.reject(error.message);
            }
        )
};

export function fetchGroup (groupId:number)
{
    return axiosInstance.get(`${axios.defaults.baseURL}/MemberGroup/${groupId}`)
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
    return axiosInstance.post(`${axios.defaults.baseURL}/MemberGroup/new`, group)
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
    return axiosInstance.post(`${axios.defaults.baseURL}/MemberGroup/member/new`, member)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function updateMember (member:Member)
{
    return axiosInstance.put(`${axios.defaults.baseURL}/MemberGroup/member/update`, member)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function deleteGroup (groupId:number)
{
    return axiosInstance.delete(`${axios.defaults.baseURL}/MemberGroup/delete/${groupId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function deleteMember (memberId:number)
{
    return axiosInstance.delete(`${axios.defaults.baseURL}/MemberGroup/member/delete/${memberId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};

export function deleteAvailability (availabilityId:number)
{
    return axiosInstance.delete(`${axios.defaults.baseURL}/MemberGroup/availability/delete/${availabilityId}`)
        .then((response:AxiosResponse)=>{
                return response.data;
            }
        )
        .catch((error:AxiosError)=>{
                return Promise.reject(error.message);
            }
        )
};