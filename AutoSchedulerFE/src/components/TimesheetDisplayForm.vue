<script setup lang="ts">
import { ref, type Ref } from 'vue';
import Button from './ui/button/Button.vue';
import { Form } from 'vee-validate';
import FormItem from './ui/form/FormItem.vue';
import { FormField } from './ui/form';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';

import Input from './ui/input/Input.vue';
import type { TimesheetViewRequirements } from '@/classes/timesheet';

const viewRequirements:TimesheetViewRequirements = {
    slotDurationInMinutes: 0,
    startTime: '0:00',
    endTime: 'T24:00Z'
};

const emit = defineEmits({
    updated(requirements:TimesheetViewRequirements){}
});
</script>

<template>
    <Form class="flex flex-row items-center justify-around m-auto my-5">
        <FormField name="startTime">
            <FormItem>
                <FormLabel>Daily Start Time</FormLabel>
                <FormControl>
                    <Input type="time" v-model="viewRequirements.startTime"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="endTime">
            <FormItem>
                <FormLabel>Daily End Time</FormLabel>
                <FormControl>
                    <Input type="time" v-model="viewRequirements.endTime"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="duration">
            <FormItem>
                <FormLabel>Slot duration in minutes</FormLabel>
                <FormControl>
                    <Input type="number" v-model="viewRequirements.slotDurationInMinutes"/>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit" @click.prevent="emit('updated', viewRequirements)">
            <slot></slot>
        </Button>
    </Form>
</template>