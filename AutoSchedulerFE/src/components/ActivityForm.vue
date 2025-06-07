<script setup lang="ts">
import { onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import { useGroupStore } from '@/stores/groupStore';
import type { Activity } from '@/classes/activity';
import Button from './ui/button/Button.vue';

const activityStore = useActivityStore();
const { activities } = storeToRefs(activityStore);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

const newActivity:Activity = {
    id: 0,
    organizationId: 0,
    title: '',
    description: ''
};

onMounted(()=>{
    activityStore.getActivitiesForOrganization(1);
});

const handleSubmit = () => {
    newActivity.organizationId=currentOrganizationIdx.value;
    activityStore.createActivity(newActivity);
};
</script>

<template>
    <Form @submit.prevent="handleSubmit">
        <FormField name="title">
            <FormItem>
                <FormLabel>Activity Title</FormLabel>
                <FormControl>
                    <Input v-model="newActivity.title" required type="text" placeholder="Accounting course"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="description">
            <FormItem>
                <FormLabel>Activity Description</FormLabel>
                <FormControl>
                    <Input v-model="newActivity.description" required type="text" placeholder="Description..."/>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Activity</Button>
    </Form>
</template>