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
import { Form } from 'vee-validate';
import Input from './ui/input/Input.vue';

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
    hallType: undefined,
    timesPerWeek: undefined,
});

defineEmits({
    created(newRequirement:ActivityRequirements){}
});
</script>

<template>
    <form @submit.prevent="$emit('created', newRequirement)">
        <Input name="duration" type="number" v-model="newRequirement.duration" required placeholder="Duration"/>
        <Input name="hallSize" type="number" v-model="newRequirement.hallsize" required="false" placeholder="Hall size"/>
        <Select v-model="newRequirement.hallType">
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
        <Select v-model="newRequirement.group">
            <SelectTrigger>
                <SelectValue placeholder="Choose group"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="group in groups" :value="group">
                    {{ group.name }}
                </SelectItem>
            </SelectContent>
        </Select>
        <Select v-model="newRequirement.activity">
            <SelectTrigger>
                <SelectValue placeholder="Choose base activity"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="activity in activities" :value="activity">
                    {{ activity.title }}
                </SelectItem>
            </SelectContent>
        </Select>
        <Button type="submit">Add</Button>
    </form>
</template>