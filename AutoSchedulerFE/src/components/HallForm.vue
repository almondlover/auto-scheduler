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
import type { Hall, HallType } from '@/classes/activity';
import { onMounted, ref, type Ref } from 'vue';
import { fetchHallTypes } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';

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
//init reactive type to sync w/ selector
const newHallType:Ref<HallType>=ref({
    id: 0,
    title: '',
    description: undefined
});

let hallTypes:HallType[];

onMounted(()=>{
    fetchHallTypes().then(types=>hallTypes=types);
})


const handleSubmit = () => {
    newHall.type = newHallType.value;
    newHall.organizationId = currentOrganizationIdx.value;
    activityStore.saveHall({...newHall});
    console.log(newHall);
};
</script>

<template>
    <Form @submit="handleSubmit">
        <FormField name="name">
            <FormItem>
                <FormLabel>Hall name</FormLabel>
                <FormControl>
                    <Input v-model="newHall.name" required type="text" placeholder="Accounting main hall"/>
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
         <FormField name="type">
            <FormItem>
                <FormLabel>Type</FormLabel>
                <FormControl>
                    <Select v-model="newHallType">
                        <SelectTrigger>
                            <SelectValue placeholder="Choose hall type"/>
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem v-for="type in hallTypes" :value="type">
                                {{ type?.title }}
                            </SelectItem>
                        </SelectContent>
                    </Select>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Hall</Button>
    </Form>
</template>