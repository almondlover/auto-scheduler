import type { Activity } from "./activity";
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
    hallName: string,
    instructor: Member | undefined,
    groups: Group[],
    dayOfTheWeek: string,
    startTime: string,
    endTime: string,
    optimizationStatus: string
};