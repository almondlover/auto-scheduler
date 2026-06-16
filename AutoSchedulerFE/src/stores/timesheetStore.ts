import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { GeneratorRequirements, Timesheet, TimesheetState, TimesheetViewRequirements, Timeslot, TimeslotPlacementChange, TimeslotRearrangement, TimeslotWeekdayTimeRanges, WeekdayTimeRange } from '@/classes/timesheet';
import type { ActivityRequirements, Hall } from '@/classes/activity';
import { activateTimesheet, createTimesheet, fetchAvailableHallsForTimeslot, fetchAvailableSpaceForTimeslot, fetchConflictingTimeslots, fetchTimesheetsForGroup, generateNewTimesheet, regenerateNewTimesheet, updateTimesheet } from '@/services/timesheetService';

export const useTimesheetStore = defineStore('timesheet', () => {
  const timesheets:Ref<Timesheet[]> = ref([]);
  const timeslots:Ref<Timeslot[]> = ref([]);
  const availableHalls:Ref<Hall[]> = ref([]);
  const currentTimesheetIdx = ref(0);
  const selectedTimeslot:Ref<Timeslot|null> = ref(null);
  const currentTimesheet = computed(()=>{return timesheets.value.find(t=>t.id==currentTimesheetIdx.value)});
  const timesheetViewConfig:Ref<TimesheetViewRequirements|null> = ref(null);
  const availableRanges:Ref<TimeslotWeekdayTimeRanges|null> = ref(null);
  async function generateTimesheet(generatorRequirements:GeneratorRequirements) {
    const generatedTimesheets:Timesheet[] = await generateNewTimesheet(generatorRequirements);
    timesheets.value=generatedTimesheets;
  }
  async function getAvailableSpaceForTimeslot(timeslotPlacementChange:TimeslotPlacementChange){
    const timeranges:WeekdayTimeRange[] = await fetchAvailableSpaceForTimeslot(timeslotPlacementChange);
    availableRanges.value = {timeslot:timeslotPlacementChange.changedTimeslot, weekdayTimeRanges:timeranges};
  }
  async function getAvailableHallsForTimeslot(timeslotPlacementChange:TimeslotPlacementChange){
    const halls:Hall[] = await fetchAvailableHallsForTimeslot(timeslotPlacementChange);
    availableHalls.value = halls;
  }
 async function getConflictingTimeslots(timeslotPlacementChange:TimeslotPlacementChange){
    const conflictingSlots:Timeslot[] = await fetchConflictingTimeslots(timeslotPlacementChange);
    timeslots.value = conflictingSlots;
  }
  async function regenerateTimesheet(timeslotRearrangement:TimeslotRearrangement) {
    const regeneratedTimesheets:Timesheet[] = await regenerateNewTimesheet(timeslotRearrangement);
    timesheets.value=regeneratedTimesheets;
  }
  async function saveTimesheet(timesheet:Timesheet) {
    const newTimesheet = await createTimesheet(timesheet);
    timesheets.value.push(newTimesheet);
  }
  async function modifyTimesheet(timesheet:Timesheet) {
    await updateTimesheet(timesheet);
    timesheets.value.splice(timesheets.value.findIndex(ts => ts.id===timesheet.id), 1, timesheet);
  }
  async function makeTimesheetActive(timesheetId:number) {
    await activateTimesheet(timesheetId);
  }
  async function resetTimesheets(){
    timesheets.value=[];
  }
  async function getTimesheetsForGroup(groupId:number, state:TimesheetState) {
    try
    {
      const timesheetsForGroup:Timesheet[] = await fetchTimesheetsForGroup(groupId, state);
      timesheets.value=timesheetsForGroup;
    }
    catch {}
  }
  return { timesheets, currentTimesheetIdx, selectedTimeslot, currentTimesheet, timesheetViewConfig, availableRanges, timeslots, availableHalls,
     getTimesheetsForGroup, generateTimesheet, getAvailableSpaceForTimeslot, getConflictingTimeslots, getAvailableHallsForTimeslot,
     saveTimesheet, resetTimesheets, regenerateTimesheet,
     modifyTimesheet, makeTimesheetActive}
})
