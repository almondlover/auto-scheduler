import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { createActivityRequirement, createHall, deleteActivity, deleteHall, fetchActivitiesForOrganization, saveActivity, updateHall } from '@/services/activityService';
import type { Activity, ActivityRequirements, Hall } from '@/classes/activity';

export const useActivityStore = defineStore('activity', () => {
  const activities:Ref<Activity[]> = ref([]);
  const currentActivityIdx = ref(0);
  const activityRequirements:Ref<ActivityRequirements[]> = ref([]);
  const halls:Ref<Hall[]> = ref([]);
  const currentActivity = computed(()=>{return activities.value.find(g=>g.id==currentActivityIdx.value)});
  async function getActivitiesForOrganization(organizationId:number) {
    activities.value = await fetchActivitiesForOrganization(organizationId);
  };
  async function createActivity(activity:Activity){
    let newActivity = await saveActivity(activity);
    activities.value.push(activity);
  };
  async function saveHall(hall:Hall){
    let newHall = await createHall(hall);
    halls.value.push(hall);
  };
  async function removeActivity(activityId:number){
    deleteActivity(activityId);
    activities.value.splice(activities.value.findIndex(act=>act.id===activityId), 1);
  };
  async function removeHall(hallId:number){
    deleteHall(hallId);
    halls.value.splice(halls.value.findIndex(hall=>hall.id===hallId), 1);
  };
  async function addRequirementForGenerator(requirement:ActivityRequirements){
    if(!activityRequirements.value.find(req=>req.id==requirement.id))
      activityRequirements.value.push(requirement);
  };
  async function removeRequirementForGenerator(requirement:ActivityRequirements){
    activityRequirements.value.splice(activityRequirements.value.indexOf(requirement), 1);
  };
  async function modifyHall(hall:Hall) {
      updateHall(hall);
      halls.value.splice(halls.value.indexOf(hall), 1);
      halls.value.push(hall);
    }
  return { activities, currentActivityIdx, currentActivity, activityRequirements, halls,
            getActivitiesForOrganization, createActivity, addRequirementForGenerator, saveHall, removeActivity, removeHall, removeRequirementForGenerator, modifyHall }
});
