<script setup lang="ts">
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import Button from './ui/button/Button.vue';
import type { Availability, Member } from '@/classes/group';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { dayOfTheWeek } from '@/constants/constants';
import ScrollArea from './ui/scroll-area/ScrollArea.vue';
import Separator from './ui/separator/Separator.vue';
import { useGroupStore } from '@/stores/groupStore';

//object to save submitted data to send to api
const newAvailability:Availability = {
    id: 0,
    startTime: '0:00',
    endTime: '0:00',
    dayOfTheWeek: 'Monday'
};

defineEmits({
    added(payload:Availability){}
})
const props = defineProps<{
    availability:Availability[]
}>();

const groupStore = useGroupStore();
</script>

<template> 
    <ScrollArea>
        <div v-for="avail in props.availability">
            <div class="flex items-center justify-between">
                {{ avail.startTime }} to {{ avail.endTime }} on {{ dayOfTheWeek[parseInt(avail.dayOfTheWeek)] }}
                <Button @click="groupStore.removeAvailability(avail.id)"> Delete </Button>
            </div>
            <Separator />
        </div>
    </ScrollArea>
    <Form class="flex items-center justify-between" @submit="$emit('added', newAvailability)">
        <FormField name="start">
            <FormItem>
                <FormLabel>Start time</FormLabel>
                <FormControl>
                    <Input v-model="newAvailability.startTime" required type="time"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="end">
            <FormItem>
                <FormLabel>End time</FormLabel>
                <FormControl>
                    <Input v-model="newAvailability.endTime" required type="time"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="day">
            <FormItem>
                <FormLabel>End time</FormLabel>
                <FormControl>
                    <Select v-model="newAvailability.dayOfTheWeek">
                        <SelectTrigger>
                            <SelectValue placeholder="Choose day of the week"/>
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem v-for="(day, i) in dayOfTheWeek" :value="i">
                                {{ day }}
                            </SelectItem>
                        </SelectContent>
                    </Select>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit">Add</Button>
    </Form>
</template>