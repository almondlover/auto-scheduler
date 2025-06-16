<script setup lang="ts">
import type { ActivityRequirements, HallType } from '@/classes/activity';
import type { Group } from '@/classes/group';
import { createActivityRequirement, fetchHallTypes } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, reactive, ref, watch, type Reactive, type Ref } from 'vue';
import Button from './ui/button/Button.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';

//initialize pinia stores
const groupStore = useGroupStore();
const { groups, members, currentOrganizationIdx } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activities, currentActivityIdx } = storeToRefs(activityStore);

let hallTypes:HallType[];

onMounted(()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    //should probably just make a request to seperate endpoint instead
    members.value = groupStore.organization(currentOrganizationIdx.value).value?.members??[];
    fetchHallTypes().then(types=>hallTypes=types);
})

watch(currentOrganizationIdx, ()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    members.value = groupStore.organization(currentOrganizationIdx.value).value?.members??[];
})

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:""},
    group: {id:0, organizationId:0, name:"", parentGroupId: 0, description:undefined, subGroups:[], requirements:[]},
    member: {id: 0, organizationId: 0, name: "", contact: "", availability:[]},
    duration: 0,
    hallsize: undefined,
    halltype: undefined,
    timesPerWeek: undefined,
}); 
</script>

<template>
    <form @submit.prevent="createActivityRequirement(newRequirement)">
        <input name="duration" type="number" v-model="newRequirement.duration" placeholder="Duration"/>
        <input name="hallSize" type="number" v-model="newRequirement.hallsize" required="false" placeholder="Hall size"/>
        <Select v-model="newRequirement.halltype">
            <SelectTrigger>
                <SelectValue placeholder="Choose hall type"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="type in hallTypes" :value="type">
                    {{ type?.title }}
                </SelectItem>
            </SelectContent>
        </Select>
        <Select v-model="newRequirement.member">
            <SelectTrigger>
                <SelectValue placeholder="Choose presenter"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="member in members" :value="member">
                    {{ member.name }}
                </SelectItem>
            </SelectContent>
        </Select>
        <select name="group" v-model="newRequirement.group">
            <option v-for="group in groups" :value="group">{{ group.name }}</option>
        </select>
        <select name="activity" v-model="newRequirement.activity">
            <option v-for="activity in activities" :value="activity">{{ activity.title }}</option>
        </select>
        <Button type="submit">Add</Button>
    </form>
</template>