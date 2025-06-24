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
import type { Member } from '@/classes/group';

const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { halls } = storeToRefs(activityStore);

//object to save submitted data to send to api
const newMember:Member = {
    id: 0,
    organizationId: undefined,
    name: '',
    contact: undefined,
    availability: []
};

const handleSubmit = () => {
    newMember.organizationId = currentOrganizationIdx.value;
    groupStore.saveMember({...newMember});
};
</script>

<template>
    <Form @submit="handleSubmit">
        <FormField name="name">
            <FormItem>
                <FormLabel>Member name</FormLabel>
                <FormControl>
                    <Input v-model="newMember.name" required type="text" placeholder="Accounting lead"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="contact">
            <FormItem>
                <FormLabel>Contacts</FormLabel>
                <FormControl>
                    <Input v-model="newMember.contact" type="text" placeholder="Contacts..."/>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Member</Button>
    </Form>
</template>