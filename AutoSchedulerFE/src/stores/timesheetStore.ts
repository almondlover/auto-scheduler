import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { fetchGroupsForOrganization } from '@/services/groupService';
import type { GeneratorRequirements, Timesheet, TimesheetViewRequirements, Timeslot, TimeslotPlacementChange, TimeslotWeekdayTimeRanges, WeekdayTimeRange } from '@/classes/timesheet';
import type { ActivityRequirements } from '@/classes/activity';
import { createTimesheet, fetchAvailableSpaceForTimeslot, fetchConflictingTimeslots, fetchTimesheetForGroup, generateNewTimesheet, regenerateNewTimesheet } from '@/services/timesheetService';

export const useTimesheetStore = defineStore('timesheet', () => {
  const timesheets:Ref<Timesheet[]> = ref([]);
  const timeslots:Ref<Timeslot[]> = ref([]);
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
 async function getConflictingTimeslots(timeslotPlacementChange:TimeslotPlacementChange){
    const conflictingSlots:Timeslot[] = await fetchConflictingTimeslots(timeslotPlacementChange);
    timeslots.value = conflictingSlots;
  }
  async function regenerateTimesheet(timeslotPlacementChange:TimeslotPlacementChange) {
    const regeneratedTimesheets:Timesheet[] = await regenerateNewTimesheet(timeslotPlacementChange);
    timesheets.value=regeneratedTimesheets;
  }
  async function saveTimesheet(timesheet:Timesheet) {
    const newTimesheet = await createTimesheet(timesheet);
    timesheets.value.push(newTimesheet);
  }
  async function resetTimesheets(){
    timesheets.value=[];
  }
  async function getTimesheetForGroup(groupId:number) {
    let timesheet = await fetchTimesheetForGroup(groupId);
    currentTimesheetIdx.value=timesheet.id;
    if (!currentTimesheet.value)
      timesheets.value.push(timesheet);
  }
  return { timesheets, currentTimesheetIdx, selectedTimeslot, currentTimesheet, timesheetViewConfig, availableRanges, timeslots,
     getTimesheetForGroup, generateTimesheet, saveTimesheet, resetTimesheets,  getAvailableSpaceForTimeslot, getConflictingTimeslots, regenerateTimesheet}
})
