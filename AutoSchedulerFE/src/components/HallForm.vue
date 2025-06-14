<script setup lang="ts">
import { storeToRefs } from 'pinia';
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import { useGroupStore } from '@/stores/groupStore';
import Button from './ui/button/Button.vue';
import { Select } from 'reka-ui/namespaced';
import { SelectContent, SelectItem, SelectTrigger, SelectValue } from 'reka-ui';
import type { Hall, HallType } from '@/classes/activity';
import { onMounted, ref } from 'vue';
import { fetchHallTypes } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';

const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { halls } = storeToRefs(activityStore);

//object to save submitted data to send to api
const newHall:Hall = {
    id: 0,
    organizationId: 0,
    name: '',
    description: undefined,
    size: 0,
    availability: undefined,
    type: {
        id: 0,
        title: '',
        description: undefined
    }
};

let hallTypes:HallType[];

onMounted(()=>{
    fetchHallTypes().then(types=>hallTypes=types);
})


const handleSubmit = () => {
    newHall.organizationId = currentOrganizationIdx.value;
    activityStore.saveHall(newHall);
};
</script>

<template>
    <Form @submit="handleSubmit">
        <FormField name="name">
            <FormItem>
                <FormLabel>Group name</FormLabel>
                <FormControl>
                    <Input v-model="newHall.name" required type="text" placeholder="Accounting team"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="description">
            <FormItem>
                <FormLabel>Description</FormLabel>
                <FormControl>
                    <Input v-model="newHall.description" required type="text" placeholder="Description..."/>
                </FormControl>
            </FormItem>
        </FormField>
         <FormField name="description">
            <FormItem>
                <FormLabel>Description</FormLabel>
                <FormControl>
                    <Select>
                        <SelectTrigger>
                            <SelectValue placeholder="Choose hall type"/>
                        </SelectTrigger>
                        <SelectItem v-for="type in hallTypes" :value="type.id">
                            {{ type.title }}
                        </SelectItem>
                    </Select>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Group</Button>
    </Form>
</template>