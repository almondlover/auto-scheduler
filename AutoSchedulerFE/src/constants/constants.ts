import { TimesheetState } from "@/classes/timesheet";

export const dayOfTheWeek = [
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday'
];

//should probably be fetched from be instead
export const roles = [
    'ResourceManager',
    'ScheduleManager'
];

export const timesheetTabsConstants =[
    {
        tabTitle: 'Active',
        title: 'Active Timesheets',
        state: TimesheetState.Active
    },
    {
        tabTitle: 'Drafts',
        title: 'Timesheet Drafts',
        state: TimesheetState.Draft
    },
    {
        tabTitle: 'History',
        title: 'Inactive Timesheets',
        state: TimesheetState.Inactive
    }
];