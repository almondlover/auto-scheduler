import type { Activity, ActivityRequirements, Hall } from "./activity"

export interface Group{
    id: number,
    organizationId: number,
    name: string,
    description: string | undefined,
    parentGroupId: number | undefined
    subGroups: Group[],
    requirements: ActivityRequirements[]
};

export interface Member{
    id: number,
    organizationId : number | undefined,
    name: string,
    contact: string | undefined,
    availability: Availability[]
};

export interface Organization{
    id: number,
    name: string,
    description: string | undefined,
    groups: Group[],
    halls: Hall[],
    activities: Activity[],
    members: Member[]
};

export interface Availability{
    id: number,
    startTime: string,
    endTime: string,
    dayOfTheWeek: string
};