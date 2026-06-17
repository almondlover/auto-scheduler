import type { Activity, ActivityRequirements, Hall } from "./activity";
import type { Group, Member } from "./group";

export enum TimesheetState{
    Draft,
    Active,
    Inactive
}

export interface Timesheet{
    id: number,
    title: string,
    state: TimesheetState,
    optimized: boolean,
    baseSlotDuration: number,
    breakSlotDuration: number,
    timeslots: Timeslot[],
    requirements: ActivityRequirements[],
    startTime: string,
    endTime: string,
    generalBreakStart: string,
    generalBreakEnd: string,
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
    breakDurationInMinutes: number,
    startTime: string,
    endTime: string,
};

export interface TimeslotPlacementChange{
    generatorRequirements: GeneratorRequirements,
    timeslotsForSheet: Timeslot[] | undefined | null,
    changedTimeslot: Timeslot
};

export interface TimeslotRearrangement{
    generatorRequirements: GeneratorRequirements,
    lockedTimeslots: Timeslot[]
};

export interface WeekdayTimeRange{
    dayOfWeek: number,
    startTime: string,
    endTime: string,
}

export interface TimeslotWeekdayTimeRanges{
    timeslot: Timeslot,
    weekdayTimeRanges:WeekdayTimeRange[]
}

export interface TimesheetViewRequirements{
    slotDurationInMinutes: number,
    breakDurationInMinutes: number,
    startTime: string,
    endTime: string
}