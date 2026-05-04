import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { fetchGroupsForOrganization } from '@/services/groupService';
import type { GeneratorRequirements, Timesheet, TimesheetViewRequirements, Timeslot } from '@/classes/timesheet';
import type { ActivityRequirements } from '@/classes/activity';
import { createTimesheet, fetchTimesheetForGroup, generateNewTimesheet } from '@/services/timesheetService';

export const useTimesheetStore = defineStore('timesheet', () => {
  const timesheets:Ref<Timesheet[]> = ref([]);
  const currentTimesheetIdx = ref(0);
  const currentTimesheet = computed(()=>{return timesheets.value.find(t=>t.id==currentTimesheetIdx.value)});
  const timesheetViewConfig:Ref<TimesheetViewRequirements|null> = ref(null);
  async function generateTimesheet(generatorRequirements:GeneratorRequirements) {
    const generatedTimesheets:Timesheet[] = await generateNewTimesheet(generatorRequirements);
    timesheets.value=generatedTimesheets;
    console.log(generatedTimesheets);
  }
  async function saveTimesheet(timesheet:Timesheet) {
    let newTimesheet = createTimesheet(timesheet);
    timesheets.value.push(timesheet);
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
  return { timesheets, currentTimesheetIdx, currentTimesheet, timesheetViewConfig, getTimesheetForGroup, generateTimesheet, saveTimesheet, resetTimesheets }
})
