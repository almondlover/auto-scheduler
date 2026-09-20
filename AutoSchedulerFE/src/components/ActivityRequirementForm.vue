<script setup lang="ts">
import type { ActivityRequirements, Hall, HallType } from '@/classes/activity';
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
import Popover from './ui/popover/Popover.vue';
import { ListboxContent, ListboxFilter, ListboxItem, ListboxItemIndicator, ListboxRoot } from 'reka-ui';
import PopoverAnchor from './ui/popover/PopoverAnchor.vue';
import PopoverTrigger from './ui/popover/PopoverTrigger.vue';
import { ChevronDown } from '@lucide/vue';
import PopoverContent from './ui/popover/PopoverContent.vue';
import { CheckIcon } from 'lucide-vue-next';
import Label from './ui/label/Label.vue';
import Checkbox from './ui/checkbox/Checkbox.vue';

//initialize pinia stores
const groupStore = useGroupStore();
const { groups, members, currentOrganizationIdx } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activities, halls } = storeToRefs(activityStore);

let hallTypes:HallType[];

onMounted(()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    activityStore.getHallsForOrganization(currentOrganizationIdx.value);
    //should probably just make a request to seperate endpoint instead
    members.value = groupStore.organization(currentOrganizationIdx.value).value?.members??[];
    fetchHallTypes().then(types=>hallTypes=types);
})

watch(currentOrganizationIdx, ()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    activityStore.getHallsForOrganization(currentOrganizationIdx.value);
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

//groups and halls for new requirement
const selectedGroups:Ref<Group[]> = ref([]);
const selectedHalls:Ref<Hall[]> = ref([]);
const hallSearch:Ref<string> = ref('');
const filteredHalls = computed(() => halls.value.filter(h => h.name.toLowerCase().includes(hallSearch.value.toLowerCase())));
const selectingHall:Ref<boolean> = ref(false);

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:"", type: undefined},
    groups: [],
    halls: [],
    combineGroups: false,
    member: {id: 0, organizationId: 0, name: "", contact: "", availability:[]},
    duration: 0,
    hallSize: undefined,
    hallType: undefined,
    timesPerWeek: undefined,
});

const rootGroups = computed(()=>groups.value.filter(g=>g.parentGroupId==null));

const emit = defineEmits({
    created(newRequirement:ActivityRequirements){}
});

const handleSubmit = ()=>{
    newRequirement.value.groups=selectedGroups.value;
    newRequirement.value.halls=selectedHalls.value;
    emit('created', newRequirement.value);
}
</script>

<template>
    <form @submit.prevent="handleSubmit">
        <h3>New Requirement for {{ newRequirement.activity.title }}</h3>
        <Label for="activity">Activity</Label>
        <Select id="activity" v-model="newRequirement.activity">
            <SelectTrigger>
                <SelectValue placeholder="Choose base activity"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem v-for="activity in activities" :value="activity">
                    {{ activity.title }}
                </SelectItem>
            </SelectContent>
        </Select>
        <Label for="duration">Activity Duration</Label>
        <Input name="duration" type="number" v-model="newRequirement.duration" required placeholder="Duration"/>
        <div class="flex items-center justify-center gap-3">
            <div class="w-1/2">
                <Label for="hallSize" class="py-2">Minimum required hall size</Label>
                <Input name="hallSize" type="number" v-model="newRequirement.hallSize" required="false" placeholder="Hall size"/>
            </div>
            <div class="w-1/2">
                <Label for="hallType" class="py-2">Hall type</Label>
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
            </div>
        </div>
        <Label for="halls">Halls</Label>
        <Popover v-model:open="selectingHall">
            <ListboxRoot
                v-model="selectedHalls"
                highlight-on-hover
                multiple
                class="w-full">
                <PopoverAnchor class="inline-flex w-full">
                    <TagsInput id="halls" v-model="selectedHalls" class="w-full">
                        <TagsInputItem v-for="hall in selectedHalls" :key="hall.id" :value="hall">
                            <TagsInputItemText>
                                {{ hall.name }}
                            </TagsInputItemText>
                            <TagsInputItemDelete />
                        </TagsInputItem>
                        <div class="w-full flex">
                            <ListboxFilter v-model="hallSearch" @update:model-value="selectingHall=hallSearch!=''" as-child>
                                <TagsInputInput placeholder="Halls for activity" @keydown.enter.prevent @keydown.down="selectingHall = true"></TagsInputInput>
                            </ListboxFilter>
                            <PopoverTrigger as-child>
                                <Button variant="ghost" class="order-last self-start ml-auto">
                                    <ChevronDown />
                                </Button>
                            </PopoverTrigger>
                        </div>
                    </TagsInput>
                </PopoverAnchor>
                <PopoverContent @open-auto-focus.prevent class="w-full">
                    <ListboxContent class="max-h-[350px] overflow-y-auto min-w-100">
                        <ListboxItem v-for="hall in filteredHalls" :value="hall" class="data-[highlighted]:bg-accent data-[highlighted]:text-accent-foreground relative flex cursor-default items-center gap-2 rounded-sm px-2 py-1.5 text-sm outline-hidden select-none data-[disabled]:pointer-events-none data-[disabled]:opacity-50"> 
                            <span>{{ hall.name }}</span>
                            <ListboxItemIndicator class="inline-flex items-center justify-center ml-auto">
                                <CheckIcon />
                            </ListboxItemIndicator>
                        </ListboxItem>
                    </ListboxContent>
                </PopoverContent>
            </ListboxRoot>
        </Popover>
        <Label for="member">Presenter</Label>
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
        <Label for="hallType">Groups</Label>
        <Select v-model="mainGroup" @update:model-value="selectedGroups=[mainGroup, ...mainGroup.subGroups]">
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
        <div class="flex gap-2">
            <Checkbox id="combineGroups" v-model="newRequirement.combineGroups"/>
            <Label for="combineGroups">Combine groups for this activity's timeslot</Label>
        </div>
        <Button type="submit">Add</Button>
    </form>
</template>