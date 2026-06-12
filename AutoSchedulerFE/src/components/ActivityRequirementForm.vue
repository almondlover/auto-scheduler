<script setup lang="ts">
import type { ActivityRequirements, HallType } from '@/classes/activity';
import type { Group } from '@/classes/group';
import { createActivityRequirement, fetchHallTypes } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { computed, onMounted, reactive, ref, watch, type Reactive, type Ref } from 'vue';
import Button from './ui/button/Button.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { Form } from 'vee-validate';
import Input from './ui/input/Input.vue';
import TabsTrigger from './ui/tabs/TabsTrigger.vue';
import TagsInput from './ui/tags-input/TagsInput.vue';
import TagsInputItem from './ui/tags-input/TagsInputItem.vue';
import TagsInputItemText from './ui/tags-input/TagsInputItemText.vue';
import TagsInputItemDelete from './ui/tags-input/TagsInputItemDelete.vue';
import TagsInputInput from './ui/tags-input/TagsInputInput.vue';

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

const mainGroup:Ref<Group> = ref({
    id: 0,
    organizationId: 0,
    name: '',
    description: undefined,
    parentGroupId: undefined,
    subGroups: [],
    requirements: []
});

const selectedGroups:Ref<Group[]> = ref([]);

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:"", type: undefined},
    groups: [],
    member: {id: 0, organizationId: 0, name: "", contact: "", availability:[]},
    duration: 0,
    hallSize: undefined,
    hallType: undefined,
    timesPerWeek: undefined,
});

const rootGroups = computed(()=>groups.value.filter(g=>g.parentGroupId==null));

defineEmits({
    created(newRequirement:ActivityRequirements){}
});
</script>

<template>
    <form @submit.prevent="newRequirement.groups=selectedGroups; $emit('created', newRequirement);">
        <h3>New Requirement for {{ newRequirement.activity.title }}</h3>
        <Input name="duration" type="number" v-model="newRequirement.duration" required placeholder="Duration"/>
        <Input name="hallSize" type="number" v-model="newRequirement.hallSize" required="false" placeholder="Hall size"/>
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
        <Select v-model="mainGroup" @update:model-value="selectedGroups=mainGroup.subGroups">
            <SelectTrigger>
                <SelectValue placeholder="Choose main group"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="group in rootGroups" :value="group">
                    {{ group.name }}
                </SelectItem>
            </SelectContent>
        </Select>
        <TagsInput v-model="selectedGroups">
            <TagsInputItem v-for="group in selectedGroups" :value="group">
                <TagsInputItemText>
                    {{ group.name }}
                </TagsInputItemText>
                <TagsInputItemDelete />
            </TagsInputItem>
            <TagsInputInput />
        </TagsInput>
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