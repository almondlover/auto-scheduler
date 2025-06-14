<script setup lang="ts">
import Card from './ui/card/Card.vue';
import type { Organization } from '@/classes/group';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardContent from './ui/card/CardContent.vue';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import GroupList from './GroupList.vue';
import HallList from './HallList.vue';
import ActivityList from './ActivityList.vue';
import MemberList from './MemberList.vue';

const props = defineProps<{
    organization:Organization
}>();
</script>

<template>
    <Card>
        <CardHeader>
            <CardTitle>{{props.organization.name}}</CardTitle>
            <CardDescription>{{ props.organization.description }}</CardDescription>
        </CardHeader>
        <CardContent>
            <Accordion type="multiple" collapsible>
                <AccordionItem value="groups">
                    <AccordionTrigger>
                        Groups
                    </AccordionTrigger>
                    <AccordionContent>
                        <GroupList/>
                    </AccordionContent>
                </AccordionItem>
                <AccordionItem value="activities">
                    <AccordionTrigger>
                        Activities
                    </AccordionTrigger>
                    <AccordionContent>
                        <ActivityList/>
                    </AccordionContent>
                </AccordionItem>
                <AccordionItem value="halls">
                    <AccordionTrigger>
                        Halls
                    </AccordionTrigger>
                    <AccordionContent>
                        <HallList/>
                    </AccordionContent>
                </AccordionItem>
                <AccordionItem value="members">
                    <AccordionTrigger>
                        Members
                    </AccordionTrigger>
                    <AccordionContent>
                        <MemberList/>
                    </AccordionContent>
                </AccordionItem>
            </Accordion>
        </CardContent>
    </Card>
</template>