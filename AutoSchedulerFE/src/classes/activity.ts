import type { Availability, Group, Member } from "./group";

export interface Activity{
    id: number,
    organizationId: number,
    title: string,
    description: string,
    type: ActivityType | undefined
};

export interface ActivityType{
    id: number,
    organizationId: number,
    title: string,
    description: string,
    baseType: ActivityType | undefined
};

export interface ActivityRequirements{
    id: number,
    activity: Activity,
    groups: Group[],
    member: Member,
    duration: number
    hallSize: number | undefined,
    hallType: HallType | undefined
};

export interface Hall{
    id: number,
    organizationId: number,
    name: string,
    description: string | undefined,
    size: number,
    availability: Availability[] | undefined,
    type: HallType
};

export interface HallType{
    id: number,
    title: string,
    description: string | undefined
}
