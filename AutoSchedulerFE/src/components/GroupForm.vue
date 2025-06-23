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
import type { Group } from '@/classes/group';
import { Select } from 'reka-ui/namespaced';
import { SelectContent } from 'reka-ui';

const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

const newGroup:Group = {
    id: 0,
    organizationId: 0,
    name: '',
    description: undefined,
    parentGroupId: undefined,
    subGroups: [],
    requirements: []
};

const handleSubmit = () => {
    newGroup.organizationId = currentOrganizationIdx.value;
    groupStore.createGroup(newGroup);
};
</script>

<template>
    <Form class="flex flex-col gap-5" @submit="handleSubmit">
        <FormField name="name">
            <FormItem>
                <FormLabel>Group name</FormLabel>
                <FormControl>
                    <Input v-model="newGroup.name" required type="text" placeholder="Accounting team"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="description">
            <FormItem>
                <FormLabel>Description</FormLabel>
                <FormControl>
                    <Input v-model="newGroup.description" required type="text" placeholder="Description..."/>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Save Group</Button>
    </Form>
</template>