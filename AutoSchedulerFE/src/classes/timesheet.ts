import type { Activity, ActivityRequirements, Hall } from "./activity";
import type { Group, Member } from "./group";

export interface Timesheet{
    id: number,
    title: string,
    active: boolean,
    optimized: boolean,
    timeslots: Timeslot[]
};

export interface Timeslot{
    id: number,
    timesheetId: number,
    activity: Activity,
    hall: Hall,
    member: Member | undefined,
    group: Group,
    dayOfWeek: number,
    startTime: string,
    endTime: string,
    optimizationStatus: string
};

export interface GeneratorRequirements{
    requirements: ActivityRequirements[],
    slotDurationInMinutes: number,
    startTime: string,
    endTime: string
};