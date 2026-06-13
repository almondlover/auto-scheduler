<script setup lang="ts">
import { onMounted, ref, watch, type Ref } from 'vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import { useGroupStore } from '@/stores/groupStore';
import type { Activity, ActivityType } from '@/classes/activity';
import Button from './ui/button/Button.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';

const activityStore = useActivityStore();
const { activityTypes } = storeToRefs(activityStore);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

onMounted(()=>{
    activityStore.getActivityTypesForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    activityStore.getActivityTypesForOrganization(currentOrganizationIdx.value);
});

const baseType:Ref<ActivityType>=ref({
    id: 0,
    organizationId: 0,
    title: '',
    description: '',
    baseTypeId: undefined,
    baseTypeName: undefined,
    subtypes: []
});
const newType:ActivityType={
    id: 0,
    organizationId: 0,
    title: '',
    description: '',
    baseTypeId: undefined,
    baseTypeName: undefined,
    subtypes: []
};
const handleSubmit = () => {
    newType.baseTypeId = baseType.value.id == 0 ? undefined : baseType.value.id;
    newType.organizationId=currentOrganizationIdx.value;
    activityStore.saveActivityType({...newType});
};
</script>

<template>
    <Form @submit="handleSubmit">
        <FormField name="title">
            <FormItem>
                <FormLabel>Activity Type Title</FormLabel>
                <FormControl>
                    <Input v-model="newType.title" required type="text" placeholder="Optional course"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="description">
            <FormItem>
                <FormLabel>Activity Type Description</FormLabel>
                <FormControl>
                    <Input v-model="newType.description" type="text" placeholder="Description..."/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="type">
            <FormItem>
                <FormLabel>Base Type</FormLabel>
                <FormControl>
                    <Select v-model="baseType">
                        <SelectTrigger>
                            <SelectValue placeholder="Choose base type"/>
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem v-for="type in activityTypes" :value="type">
                                {{ type?.title }}
                            </SelectItem>
                        </SelectContent>
                    </Select>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Activity Type</Button>
    </Form>
</template>