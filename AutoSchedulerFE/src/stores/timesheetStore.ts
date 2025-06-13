import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { fetchGroupsForOrganization } from '@/services/groupService';
import type { GeneratorRequirements, Timesheet } from '@/classes/timesheet';
import type { ActivityRequirements } from '@/classes/activity';
import { generateNewTimesheet } from '@/services/timesheetService';

export const useTimesheetStore = defineStore('timesheet', () => {
  const timesheets:Ref<Timesheet[]> = ref([]);
  const currentTimesheetIdx = ref(0);
  const currentTimesheet = computed(()=>{return timesheets.value.find(t=>t.id==currentTimesheetIdx.value)});
  async function generateTimesheet(generatorRequirements:GeneratorRequirements) {
    const timesheet = await generateNewTimesheet(generatorRequirements);
    timesheets.value.push(timesheet);
  }
  return { timesheets, currentTimesheetIdx, currentTimesheet, generateTimesheet }
})
