import type { Availability, Group, Member } from "./group";

export interface Activity{
    id: number,
    organizationId: number,
    title: string,
    description: string,
};

export interface ActivityRequirements{
    id: number,
    activity: Activity,
    group: Group,
    member: Member,
    duration: number
    hallsize: number | undefined,
    halltype: HallType | undefined
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
