﻿using System;

namespace EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent() {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid id, DateTime createdDate) {
            Id = id;
            CreatedDate = createdDate;
        }

        public Guid Id { get; private set; }

        public DateTime CreatedDate { get; private set; }
    }
}