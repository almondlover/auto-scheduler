import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { createActivityRequirement, fetchActivitiesForOrganization, saveActivity } from '@/services/activityService';
import type { Activity, ActivityRequirements } from '@/classes/activity';

export const useActivityStore = defineStore('activity', () => {
  const activities:Ref<Activity[]> = ref([]);
  const currentActivityIdx = ref(0);
  const generatorActivityRequirements:Ref<ActivityRequirements[]> = ref([]);
  const activityRequirements:Ref<ActivityRequirements[]> = ref([]);
  const currentActivity = computed(()=>{return activities.value.find(g=>g.id==currentActivityIdx.value)});
  async function getActivitiesForOrganization(organizationId:number) {
    activities.value = await fetchActivitiesForOrganization(organizationId);
  };
  async function createActivity(activity:Activity){
    let newActivity = await saveActivity(activity);
    activities.value.push(activity);
  };
  async function addRequirementForGenerator(requirement:ActivityRequirements){
    activityRequirements.value.push(requirement);
  }
  return { activities, currentActivityIdx, currentActivity, activityRequirements, getActivitiesForOrganization, createActivity, addRequirementForGenerator }
});
