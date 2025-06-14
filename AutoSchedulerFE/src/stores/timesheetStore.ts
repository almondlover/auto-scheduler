import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { fetchGroupsForOrganization } from '@/services/groupService';
import type { GeneratorRequirements, Timesheet, Timeslot } from '@/classes/timesheet';
import type { ActivityRequirements } from '@/classes/activity';
import { createTimesheet, generateNewTimesheet } from '@/services/timesheetService';

export const useTimesheetStore = defineStore('timesheet', () => {
  const timesheets:Ref<Timesheet[]> = ref([]);
  const currentTimesheetIdx = ref(0);
  const currentTimesheet = computed(()=>{return timesheets.value.find(t=>t.id==currentTimesheetIdx.value)});
  async function generateTimesheet(generatorRequirements:GeneratorRequirements) {
    const generatedTimeslots:Timeslot[][] = await generateNewTimesheet(generatorRequirements);
    //should all be handled in be
    let generatedTimesheets:Timesheet[]=[];
    for (const timeslotCollection in generatedTimeslots)
    {
      let newTimesheet:Timesheet={
        id: 0,
        title: '',
        active: false,
        optimized: false,
        timeslots: []
      };
      generatedTimesheets.push(newTimesheet);
      newTimesheet.timeslots = generatedTimeslots[timeslotCollection]
    }
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
  return { timesheets, currentTimesheetIdx, currentTimesheet, generateTimesheet, saveTimesheet, resetTimesheets }
})
