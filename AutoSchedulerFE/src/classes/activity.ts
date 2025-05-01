import type { Group } from "./group";

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
    duration: number
    hallsize: number | undefined,
    halltype: string | undefined,
    timesPerWeek: number | undefined
};

